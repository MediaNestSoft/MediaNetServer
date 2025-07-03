namespace MediaNetServer.Models;

public class LocalGetSeriesDetail200Response
{
    public LocalSeriesDetail? SeriesDetail { get; set; }
    public LocalGetSeriesDetail200Response(LocalSeriesDetail seriesDetail)
    {
        SeriesDetail = seriesDetail;
    }


    public LocalGetSeriesDetail200Response() { }
}