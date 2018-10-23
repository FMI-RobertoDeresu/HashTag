from .vgg19_model import VGG19_Model
VGG19_Model.initialization()

from django.core.wsgi import get_wsgi_application
application = get_wsgi_application()