import inspect, os
import cv2
import numpy as np

class ImageUtils:
    @staticmethod
    def resizeImage(photo, scaleX, scaleY):
        resizedPhoto = cv2.resize(photo, (scaleX, scaleY)).astype(np.float32)
        resizedPhoto[:,:,0] -= 103.939
        resizedPhoto[:,:,1] -= 116.779
        resizedPhoto[:,:,2] -= 123.68
        resizedPhoto = resizedPhoto.transpose((2,0,1))
        
        return resizedPhoto