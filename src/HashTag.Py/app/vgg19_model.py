import inspect, os
from keras.models import Sequential
from keras.layers.core import Flatten, Dense, Dropout
from keras.layers.convolutional import Convolution2D, ZeroPadding2D
from keras.layers.pooling import MaxPooling2D
from keras.optimizers import SGD
import cv2
import numpy as np
from .imageUtils import ImageUtils

class VGG19_Model:
    model = Sequential()
    
    @staticmethod
    def CreateModel(weights_path=None):
        model = Sequential([
            ZeroPadding2D((1,1),input_shape=(3, 224, 224)),
            Convolution2D(64, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(64, 3, 3, activation='relu'),
            MaxPooling2D((2,2), strides=(2,2)),

            ZeroPadding2D((1,1)),
            Convolution2D(128, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(128, 3, 3, activation='relu'),
            MaxPooling2D((2,2), strides=(2,2)),

            ZeroPadding2D((1,1)),
            Convolution2D(256, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(256, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(256, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(256, 3, 3, activation='relu'),
            MaxPooling2D((2,2), strides=(2,2)),

            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            MaxPooling2D((2,2), strides=(2,2)),

            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            ZeroPadding2D((1,1)),
            Convolution2D(512, 3, 3, activation='relu'),
            MaxPooling2D((2,2), strides=(2,2)),

            Flatten(input_shape=(224, 224)),
            Dense(4096, activation='relu'),
            Dropout(0.5),
            Dense(4096, activation='relu'),
            Dropout(0.5),
            Dense(1000, activation='softmax')
        ])
        
        if weights_path:
            model.load_weights(weights_path)

        sgd = SGD(lr=0.1, decay=1e-6, momentum=0.9, nesterov=True)
        model.compile(optimizer=sgd, loss='categorical_crossentropy')

        return model

    @staticmethod
    def initialization():
        print ("Creating model..")
        path = os.path.dirname(os.path.abspath(inspect.getfile(inspect.currentframe())))
        weights_path = os.path.join(path, 'vgg19_weights.h5')
        VGG19_Model.model = VGG19_Model.CreateModel(weights_path)
        print ("Ready!")

    @staticmethod
    def getPredictedData(photosPaths):
        photos = []
        for photoPath in photosPaths:
            photo = ImageUtils.resizeImage(cv2.imread(photoPath), 224, 224)
            photos.append(photo)

        return VGG19_Model.model.predict(np.array(photos), 32, 1)