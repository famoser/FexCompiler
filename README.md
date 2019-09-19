# FexCompiler

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/987fgimtc5rk546d?svg=true)](https://ci.appveyor.com/project/famoser/fexcompiler)
[![codecov](https://codecov.io/gh/famoser/FexCompiler/branch/master/graph/badge.svg)](https://codecov.io/gh/famoser/FexCompiler)

Fex was created specifically to write summaries. 
Its setup helps you to structure content effectively and then allows to generate a printable .pdf and learning cards.

It is awesome because:

- (re-)structure content easily & fast
- do it in the text editor of your choice
- export it for printing or flash cards

There is no support for images, math or other non-text content; use your own words!

### Basic idea

There are only two rules:

- You may describe a concept on a single line (like `fex: a format to write summaries` or you may use multiple lines (indented by tabs).
- You may do this recusively.

For example:

```
.fex:
	format to write summaries faster than ever before
	export formats:
		.pdf to print your summary
		.xlsx to import the concepts of your summary into a flash card service
```

### Export formats

- **.pdf**: optimized for printing.

if you use the file ending `.l.fex`, additionally:

- **.json**: used it with https://github.com/famoser/FexFlashcards to memorize your summary with flash card functionality.
- **.xlsx**: collection of your concepts so you can import then in a flash card service.


### Real-World Example

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