namespace MediaNetServer.Models;

public class LocalGetMovieDetail200Response
{
    public LocalMovieDetail? MovieDetail { get; set; }
    public LocalGetMovieDetail200Response(LocalMovieDetail movieDetail)
    {
        MovieDetail = movieDetail;
    }


    public LocalGetMovieDetail200Response() { }
}