namespace Wallet.External.Nbp.Dto;

public record NbpTable(string table, string no, DateOnly effectiveDate, List<NbpRate> rates);