# Fquery

A cli utility to query the underlying filesystem for files with a DSL inspired by SQL.

## Examples
```shell
match("*.cs") AND includes("public", > 3)
```
A command which would get all files ending in ``.cs`` and having more than 3 occurences of the word ``public``