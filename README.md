# FexCompiler

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/987fgimtc5rk546d?svg=true)](https://ci.appveyor.com/project/famoser/fexcompiler)
[![codecov](https://codecov.io/gh/famoser/FexCompiler/branch/master/graph/badge.svg)](https://codecov.io/gh/famoser/FexCompiler)

The FexCompiler processes .fex files to create various useful files like a printable pdf or a flash card collection.

## Fex
### Motivation
Fex was created specifically for fast creation of summaries in a text editor.  There is no support for images, math or other non-text content to force the user to formulate out everything.

### Basic idea
The summary is broken down into concepts and their descriptions. You may create concepts which are described again as a collection of concepts. You may describe a concept on a single line (like `fex: a format to write summaries` or you may use multiple lines, and use tabs to define affiliation. For example:

```
why you should use .fex:
	your summaries are written faster than ever before
	various export formats are available
```

### Why fex is awesome
- (re-)structuring of content is easy and fast to do.
- faster to write than using word or latex
- helps you to structure content effectively
- helps you to explain a concept short and concise

### Export formats
- **.pdf**: optimized for printing. As a side effect forces you to keep descriptions short as else the introduced line breaks look ugly
- **.json**: used it with https://github.com/famoser/FexFlashcards to memorize your summary with flash card functionality
- **.xlsx**: collection of your concepts so you can import then in a flash card service


### Example
The fex format in a real world setting

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
(except from https://github.com/famoser/eth-summaries/blob/master/2017-2%20Concepts%20of%20Object-Oriented%20Programming.fex)
