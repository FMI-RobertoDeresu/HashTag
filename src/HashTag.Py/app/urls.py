from django.conf.urls import url
from . import views

urlpatterns = [
    url(r'^api/processphotos', views.processPhotos, name='api/processPhotos')
]