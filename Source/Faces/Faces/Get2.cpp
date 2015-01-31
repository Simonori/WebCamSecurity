
#include <stdio.h>
#include <stdlib.h>


#include <iostream>


#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/objdetect/objdetect.hpp"
using namespace cv;


static CvMemStorage* Storage = NULL;
static CvHaarClassifierCascade* Cascade = NULL;
static CvMemStorage* AltStorage = NULL;
static CvHaarClassifierCascade* AltCascade = NULL;


static long FileNumber = 0;
static long Recordnumber = 0;
bool Store ;
short Speed = 75;
char* DirectoryBuffer;

void DetectAndDraw( IplImage* Image );
IplImage* CompareImages( IplImage*, IplImage*);

int main( int argc, char** argv )
{
	char dest[256];
	strcpy( dest, argv[3]);
	char* CascadeName = strcat(dest , "haarcascade_frontalface_alt.xml");
	char* AltCascadeName = strcat(argv[3] , "haarcascade_profileface.xml");
	std::cout<<CascadeName<< std::endl;
	std::cout<<AltCascadeName<< std::endl;
	std::cout<<argv[2]<< std::endl;
	
	if ( argv[ 2 ] == "1" ) Speed = 30;
	else if ( argv[ 2 ] == "2" ) Speed = 100;
	else Speed = 60;
	
	CvCapture* Capture = NULL;
	IplImage* Frame = NULL;
	Cascade = ( CvHaarClassifierCascade* )cvLoad( CascadeName, 0, 0, 0 );
	AltCascade = ( CvHaarClassifierCascade* )cvLoad( AltCascadeName, 0, 0, 0 );
	Storage = cvCreateMemStorage( 0 );
	AltStorage = cvCreateMemStorage( 0 );
	Capture = cvCaptureFromCAM(0);
	DirectoryBuffer = argv[ 1 ];

	
	if ( Capture )
	{
		IplImage* OldFrame = NULL;
		IplImage* ResultFrame = NULL;
		long Count = 0;
		Store = true;
		cvNamedWindow( "Face", CV_WINDOW_AUTOSIZE );
		while ( cvWaitKey( 43 ) != 27 )
		{
			
			Frame = cvQueryFrame( Capture );
			DetectAndDraw( Frame );
			
		}
	}
	cvReleaseImage( &Frame );
	cvDestroyWindow( "Face" );
	cvReleaseCapture( &Capture );
	cvReleaseMemStorage( &Storage );
	cvReleaseMemStorage( &AltStorage );
	
	return 0;
}
void DetectAndDraw( IplImage* Img )
{
	int p[3];
	p[0] = CV_IMWRITE_JPEG_QUALITY;
    p[1] = 80;
    p[2] = 0;

	char Filename [ 256 ];
	bool WriteFlag = false;
	short Scale = 1;
	
	CvPoint Pt1;
	CvPoint Pt2;
	
	Recordnumber++;
	cvClearMemStorage( Storage );
	cvClearMemStorage( AltStorage );
	if ( Cascade )
	{
		CvSeq* Faces = cvHaarDetectObjects( Img, Cascade, Storage, 1.1, 2, CV_HAAR_DO_CANNY_PRUNING, cvSize( 40, 40 ) );
		CvSeq* SideFaces = cvHaarDetectObjects( Img, AltCascade, AltStorage, 1.1, 2, CV_HAAR_DO_CANNY_PRUNING, cvSize( 40, 40 ) );
		if ( Faces->total > 0 || SideFaces->total > 0 ) 
		{
			WriteFlag = true;
			if ( Recordnumber > Speed )
			{
				Store = true;
				Recordnumber = 0;
			}
		}
	
		for ( int Count = 0; Count < (Faces ? Faces->total : 0 ); Count++ )
		{
			CvRect* Rect = ( CvRect* )cvGetSeqElem( Faces, Count );
			Pt1.x = Rect->x * Scale;
			Pt2.x = ( Rect->x + Rect->width )* Scale;
			Pt1.y = Rect->y * Scale;
			Pt2.y = ( Rect->y + Rect->height ) * Scale;
			cvRectangle( Img, Pt1, Pt2, CV_RGB( 0, 0, 255 ), 1, 8, 0 );
		}
		for ( int Count = 0; Count < (SideFaces ? SideFaces->total : 0 ); Count++ )
		{
			CvRect* Rect = ( CvRect* )cvGetSeqElem( SideFaces, Count );
			Pt1.x = Rect->x * Scale;
			Pt2.x = ( Rect->x + Rect->width )* Scale;
			Pt1.y = Rect->y * Scale;
			Pt2.y = ( Rect->y + Rect->height ) * Scale;
			cvRectangle( Img, Pt1, Pt2, CV_RGB( 0, 255, 0 ), 1, 8, 0 );
		}
	}
	if ( WriteFlag && Store )
	{
		int p[3];
		p[0] = CV_IMWRITE_JPEG_QUALITY;
   		p[1] = 10;
    	p[2] = 0;

		FileNumber++;
		sprintf( Filename, "%s/Image%ld.jpg", DirectoryBuffer, FileNumber );
		std::cout<<Filename<< std::endl;
		
		cvSaveImage( Filename, Img, p);
		Store = false;
	}
	if ( Recordnumber > 999999999 ) Recordnumber = 0;
	cvShowImage( "Face", Img );
}

/*void Checker( CString DirectoryBuffer, CString& Stop )
{
	CString Stopped = "22";
	CString FileDire = "";
	char StopFile [ 256 ];
	char* FBuff ="22";
	sprintf( StopFile, "%s\\Stopper.txt", DirectoryBuffer );
	FileDire = StopFile;
	FileDire.Replace( "\\","\\\\" );
	FILE* pStop = fopen( StopFile, "rb" );
	if ( pStop )
	{
		fseek( pStop, 0, SEEK_END );
		long Lsize = ftell( pStop );
		rewind( pStop );
		FBuff = ( char* ) malloc( sizeof( char ) * Lsize );
		size_t Result = fread( FBuff, 1, Lsize, pStop );
		fclose( pStop );
		
	}
	
	Stopped = FBuff;
	Stopped = Stopped.Left( 2 );
	Stop = Stopped;

}
IplImage* CompareImages(IplImage * Climg1, IplImage * Climg2) //Compare two images (frame)  
{  
     //this->clear(); //Clears image buffers.  
   
     //Check if it's images  
    //if ((img1 == 0) || (img2 == 0)) { fprintf(stderr, "One/both of images to compare are null\n"); return; }  
   
     //Clone image in order not to modify the original  
     //IplImage* Climg1 = cvCloneImage(img1);  
     //IplImage* Climg2 = cvCloneImage(img2);  
   
     //Create processed image buffer  
      IplImage* processedImg = cvCreateImage(cvGetSize(Climg1), 8, 3 );
   
     //Blur images to get rid of camera noise  
     cvSmooth(Climg1, Climg1, CV_BLUR, 3);  
     cvSmooth(Climg2, Climg2, CV_BLUR, 3);   
   
     //Calc absolute difference  
     cvAbsDiff(Climg1, Climg2, processedImg);  
   
     //Create gray image buffer  
     IplImage* processedImgGray = cvCreateImage(cvGetSize(processedImg), 8, 1);  
   
     //Convert colored image to grayscale  
     cvCvtColor(processedImg, processedImgGray, CV_RGB2GRAY);  
   
     //Perform binary treshold filter on image to leave only white and black pixels  
     cvThreshold(processedImgGray, processedImgGray, 30, 255, CV_THRESH_BINARY);  
    return processedImgGray;
 }  */
