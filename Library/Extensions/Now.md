# Now: get the current date and time

The **Now** block reports the current date and time. In regular mode the value is formatted to a string and can be used for generating timestamps. If the format is set to _null_ the internal .NET integral number representation is reported which can be used for time measurements.

[Back to overview](index.md)

## FORMAT Parameter (String)

If a string is provided the date and time is formatted according to the [.NET rules](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings). The block provides a shadow block for the FORMAT using the value _dd.MM.yyyy HH:mm:ss_.

If FORMAT is explicitly set to _null_ the internal time and date represenation is reported. This resolution is [10 million counts per second](https://learn.microsoft.com/en-us/dotnet/api/system.datetime.ticks?view=net-8.0).

## Returns

The current date time either as a formatted string or some internal number representation.

## Blockly editor

![Now block in the blockly editor](Now.png)
