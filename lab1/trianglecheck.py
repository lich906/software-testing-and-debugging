import sys

def tryConvertToIntOrFloat(valueList):
    res = []
    for varString in valueList:
        convertedVal = None
        try:
            convertedVal = int(varString)
        except ValueError:
            convertedVal = float(varString)
        res.append(convertedVal)
    return res

def checkArgsCount(args, requiredCount):
    return len(args) == requiredCount

#----------------------------------------

argv = sys.argv
del argv[0]

if not checkArgsCount(argv, 3):
    print("неизвестная ошибка")
    exit(0)

try:
    argv = tryConvertToIntOrFloat(argv)
except ValueError:
    print("неизвестная ошибка")
    exit(0)

a, b, c = argv
if (a >= b + c) or (b >= a + c) or (c >= a + b):
    print("не треугольник")
elif (a == b and b == c):
    print("равносторонний")
elif (a == b or b == c or a == c):
    print("равнобедренный")
else:
    print("обычный")
            
