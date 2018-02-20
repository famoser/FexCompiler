# FexCompiler

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/987fgimtc5rk546d?svg=true)](https://ci.appveyor.com/project/famoser/fexcompiler)
[![codecov](https://codecov.io/gh/famoser/FexCompiler/branch/master/graph/badge.svg)](https://codecov.io/gh/famoser/FexCompiler)

The FexCompiler can process .fex files to create a printable .pdf.

# Fex

Fex was created specifically for fast creation of summaries in a text editor. 
Its very simple:  
Write each topic on its own line, and use tabs to differentiate chapters.
You may also use the structure "keyword: content" if you don't want to create a new line for a few words.

## Example Fex
except from https://github.com/famoser/eth-summaries/blob/master/2017-2%20Concepts%20of%20Object-Oriented%20Programming.fex to demonstrate the fex format in a real world setting

```
language concepts
	enable and facilitate the application of core concepts
	dynamic method binding:
		enables classification and polymorphism
		method implementation is selected at runtime
	why not just use language concepts as guidelines:
		inhertiance has been replaced by code duplication
		subtyping needed casts, same memory layout of super & subclasses needed
```
