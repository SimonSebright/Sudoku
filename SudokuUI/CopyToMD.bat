del "D:\SudokuTest\*.*" /F /S /Q
RmDir /S /Q D:\SudokuTest
MkDir D:\SudokuTest


copy bin\Release\*.exe D:\SudokuTest
copy bin\Release\*.dll D:\SudokuTest

md D:\SudokuTest\Examples
Copy Examples\*.* D:\SudokuTest\Examples
