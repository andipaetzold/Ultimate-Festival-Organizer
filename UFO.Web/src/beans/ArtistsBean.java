package beans;

import services.Artist;
import services.UltimateFestivalOrganizer;
import services.UltimateFestivalOrganizerSoap;

import javax.faces.bean.ManagedBean;
import javax.faces.bean.SessionScoped;
import javax.faces.context.FacesContext;
import java.util.List;
import java.util.Map;

@ManagedBean(name = "artistsBean")
@SessionScoped
public class ArtistsBean {
    private Artist artist;

    public List<Artist> getAll() {
        UltimateFestivalOrganizer service = new UltimateFestivalOrganizer();
        UltimateFestivalOrganizerSoap ufo = service.getUltimateFestivalOrganizerSoap();

        return ufo.getAllButDeletedArtists().getArtist();
    }

    public void updateDialog() {
        Map<String, String> requestParams = FacesContext.getCurrentInstance().getExternalContext().getRequestParameterMap();
        if (!requestParams.containsKey("id")) {
            return;
        }
        int id = Integer.valueOf(requestParams.get("id"));

        UltimateFestivalOrganizer service = new UltimateFestivalOrganizer();
        UltimateFestivalOrganizerSoap ufo = service.getUltimateFestivalOrganizerSoap();
        artist = ufo.getArtistById(id);
    }

    public Artist getArtist() {
        return artist;
    }
}
