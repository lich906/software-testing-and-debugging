import subprocess
import sys

def checkArgsCount(args, required_count):
    if len(args) != required_count:
        print("invalid arguments")
        print("\tusage: tester.py testcases_file program_to_test test_results_file")
        exit(0)

#---------------------------------------

argv = sys.argv
del argv[0]

checkArgsCount(argv, 3)

testcases_file_path, program_under_test_path, test_results_file_path = argv

with open(testcases_file_path, "r") as testcases_file:
    testcases = testcases_file.readlines()

with open(test_results_file_path, "w") as test_results_file:
    for testcase in testcases:
        if testcase == "":
            continue
        #a, b, c, expected_result = testcase.rstrip().split(',')
        testcase_data = testcase.rstrip().split(':')
        expected_result = testcase_data[1]
        cmd = ' '.join([program_under_test_path] + testcase_data[0].split(','))
        process = subprocess.Popen(cmd, stdout=subprocess.PIPE, shell=True)
        out, err = process.communicate()
        result = out.rstrip().decode('cp1251')
        if result == expected_result:
            test_results_file.write("success\n")
        else:
            test_results_file.write("error\n")
