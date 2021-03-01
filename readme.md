# Cash Register
A simple command line utility to calculate change due.
# Rules
Normaly change is calculated to prefer larger denominations unless the change due is evenly divisible by 3 cents.  In that case, the correct change is made with a random distribution of denominations (note that this is not a deterministic results and multiple runs with the same input is expected to produce different output).
# Running
To run the program and produce sample output to the console run `run-sample.bat`.

To run unit tests run `test.bat`.

To build locally and run with arbitrary input, run `build.bat` then run `.\bin\CashRegister.exe`.  By default, output is only written to the console.  You can write to an output file by specifying the `-o` command line option with a file to write to.