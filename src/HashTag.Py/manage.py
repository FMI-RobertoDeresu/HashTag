import os
import sys

if __name__ == "__main__":
    print sys.argv

    from django.core.management import execute_from_command_line
    execute_from_command_line(sys.argv)