namespace HttpAgent.Samples;

public class ClayContentConverter : HttpContentConverterBase<Clay>
{
    /// <inheritdoc />
    public override Clay? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        var str = httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();

        return Clay.Parse(str);
    }

    /// <inheritdoc />
    public override async Task<Clay?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        var str = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return Clay.Parse(str);
    }
}