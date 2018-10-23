import os
import sys
import keras

if __name__ == "__main__":
    os.environ['KERAS_BACKEND'] = 'theano'
    keras.backend.set_image_dim_ordering("th")

    from django.core.management import execute_from_command_line
    execute_from_command_line(sys.argv)