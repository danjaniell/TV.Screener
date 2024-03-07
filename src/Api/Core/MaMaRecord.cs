namespace Api.Core;

public record MaMaRecord(
    DateTime Date,
    double AlmaValue,
    double MacdValue,
    double MacdSignal,
    double MacdHistogram
);
