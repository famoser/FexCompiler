# FexCompiler

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/987fgimtc5rk546d?svg=true)](https://ci.appveyor.com/project/famoser/fexcompiler)
[![codecov](https://codecov.io/gh/famoser/FexCompiler/branch/master/graph/badge.svg)](https://codecov.io/gh/famoser/FexCompiler)

Fex helps your to write and restructure rapidly. 
Its primary purpose is to write summaries of books and courses. 

The format works indentation-based:
```
.fex:
	format to write summaries faster than ever before
	export formats:
		.pdf to print your summary
		.md for publishing in the web
		.xlsx or .json to import into a flash card service
```

You can grasp in an instant in which chapter you are, and how this chapter relates to others in terms of abstraction level. 
Restructuring is blazingly fast: You only need to change the indentation (which nearly all editors have shortcuts for).

The `.pdf` export uses latex under the hood; hence math expressions are possible. Simple expressions are even detected automatically so you do not have to write the cumbersome latex syntax yourself.

There is no support for images or other non-text content; use your own words!

## Export formats

Available exports are:
- **.pdf**: optimized for printing.
- **.md**: publish in the web
- **.json**: use it with https://github.com/famoser/FexFlashcards to memorize your summary with flash card functionality.
- **.xlsx**: collection of your concepts so you can import then in a flash card service.

You can declare exports before the `.fex` ending. For example, `summary.md-json.fex` will generate `summary.md` and `summary.json`. If you do not declare anything, then a PDF is generated.

## Real-World Example

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
(excerpt from https://github.com/famoser/eth-summaries/blob/master/2017-2%20Concepts%20of%20Object-Oriented%20Programming.fex)

## Advanced features

Some advanced features:
- You can use triple-` to start and end a codeblock
- Primary and secondary headers can be denoted by an immediately following line of `===` resp. `---` (at least 3 chars)