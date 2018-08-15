from PyPoE.poe.file.specification.data import stable
from json import dump
with open('./spec.json', 'w') as f:
    dump(stable.specification.as_dict(), f)