#import dependencies
import cv2
import argparse
#construct argument parses
ap = argparse.ArgumentParser()
ap.add_argument("-i","--image", required=True,help="path to input image")
args = vars(ap.parse_args())
#preload the image into OpenCV
image = cv2.imread(args["image"])
saliency = cv2.saliency.StaticSaliencySpectralResidual_create()
(success, saliencyMap) = saliency.computeSaliency(image)
saliencyMap = (saliencyMap * 255).astype("uint8")

#cv2.imshow("Image", image)
print ("input file path: "+args["image"]);
filename = (args["image"].split("\\")[6]).split(".")[0]
#cv2.imwrite(filename+"_rawImage_"+".jpg",image)
#cv2.imshow("Output", saliencyMap)
saliency = cv2.saliency.StaticSaliencyFineGrained_create()
(success, saliencyMap) = saliency.computeSaliency(image)
threshMap = cv2.threshold(saliencyMap.astype("uint8"), 0, 255,
	cv2.THRESH_BINARY | cv2.THRESH_OTSU)[1]
# show the images
#cv2.imshow("Image", image)
cv2.imwrite(filename+"_rawImage_"+".jpg",image)
#cv2.imshow("Output", saliencyMap)
cv2.imwrite(filename+"_staticSpectral_"+".jpg",saliencyMap)
#cv2.imshow("Thresh", threshMap)
cv2.imwrite(filename+"_threshMap_"+".jpg",threshMap)
cv2.waitKey(0)
