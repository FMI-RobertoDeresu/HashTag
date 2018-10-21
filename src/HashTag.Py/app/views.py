from rest_framework.decorators import api_view
from rest_framework.response import Response
import threading
from datetime import datetime
from vgg19_model import VGG19_Model

lock = threading.Lock()

@api_view(['POST'])
def processPhotos(request):
    with lock:
        photosPaths = request.data['photosPaths']
        
        startTime = datetime.now()
        print('Start time ' + str(startTime))

        response = VGG19_Model.getPredictedData(photosPaths)

        endTime = datetime.now()
        print('End time ' + str(endTime))
        
        totaTime = (endTime - startTime).seconds + (endTime - startTime).microseconds/1E6
        print('Total time ' + str(totaTime) + 's\n\n')

        return Response(response)