namespace Api.Core;

public record MaMaRecord(
    DateTime Date,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    double AlmaValue,
    double MacdValue,
    double MacdSignal,
    double MacdHistogram
);
