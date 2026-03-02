using System;

namespace ShipIt.PriceQuote.Service;

public class PriceQuoteExceptionNLWeightExceeded(string message) : Exception(message){}
