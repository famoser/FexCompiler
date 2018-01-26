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

```
philosophical concepts
	matter of counting lives, weigthing cost & benefit & consequences? -> Utilarism
	or are some morals higher than calculations and must be protected at all cost?
	Utilarismus:
		Bentham: The highest principle of morality is to maximize happiness, the overall balance of pleasure over pain.
		Mill: what will maximize welfare or the collective happiness of society as a whole. It is the result that counts. 
	Kant:
		Motive counts, not result
		Categorical Imperative (universal law): "Act only according to the maxim whereby you can, at the same time, will that it should become a universal law"
	Aufklärung ist der Ausgang des Menschen aus seiner selbst verschuldeten Unmündigkeit
		Selbsverschuldet weil nicht wegen mangel an Intellekt
		Unmündig weil nicht nachdenken ohne Hilfe von Drittpersonen
		Sapere ade
```
