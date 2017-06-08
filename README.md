# FexCompiler


This projects compiles .fex files to .pdf files. It first convertes the .fex format to .tex, and then calls a .tex compiler to create the pdf. You can find the specifications of the .fex language down below.

## Fex

This language was created specifically for the easy & fast creation of summaries in a text editor, and then convert it to a printable format. 
The language has no control caracters, and builds the .tex mainly from tabs & spaces.

Following are the currently supported caracters:
 - The Tab caracter creates a new chapter. The first line of the new chapter is its title
 - You can create lists with either `-` or numbers in the format `1:`
 - You can create bold text with appendending a `:` at the end (example: `Diffusion:`)

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