import vgg19_model
vgg19_model.VGG19_Model.initialization()

from django.core.wsgi import get_wsgi_application
application = get_wsgi_application()