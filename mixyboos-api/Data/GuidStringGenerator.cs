using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace MixyBoos.Api.Data;

public class GuidStringGenerator : ValueGenerator<string> {
  private readonly SequentialGuidValueGenerator _guidGenerator = new();

  public override bool GeneratesTemporaryValues
    => false;

  public override string Next(EntityEntry entry) {
    return _guidGenerator.Next(entry).ToString();
  }
}
