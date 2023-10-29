import React from 'react';
import { useState } from 'react';
import { SetURLSearchParams } from "react-router-dom";
import { Map, Marker, Overlay, ZoomControl } from 'pigeon-maps'
import { useFetch, useLocalStorage } from '../../customHooks';
import { LocalStorageParams, Tabs, URLParams } from '../../constants';
import { IMapState, INoteDTO, IUserDTO } from '../../types';
import { CloseIcon } from '../icons/icons';
import { convertStringToInt } from '../../utils';

const DEFAULTMAPSTATE: IMapState = {
  zoom: 10, 
  long: 76.91850, 
  lat: 43.24980
}

const getMarkersRequest = (mapState: IMapState, query: string | null, tagSum: number, user: IUserDTO | null | undefined): string | null => {
  if (user){
    return `api/mapnotes?user=${user.name}&zoom=${mapState.zoom}&lon=${mapState.long}&lat=${mapState.lat}&query=${query || ''}&tags=${tagSum}`;
  }
  return null;
};

const getCenter = (note: INoteDTO | null | undefined): [number, number] => {
  if (note && note.isMark === true){
    return [note.latitude, note.longitude];
  }
  return [DEFAULTMAPSTATE.lat, DEFAULTMAPSTATE.long];
}

const useSelectedNote = <T,>(propsValue: T): [T, React.Dispatch<React.SetStateAction<T>>] => {
  const [value, setValue] = useState<T>(propsValue);

  React.useEffect(() => {
    setValue(propsValue);
  }, [propsValue]);

  return [value, setValue];
};

const usePopupDisplay = (propsValue: INoteDTO | null | undefined): [string, React.Dispatch<React.SetStateAction<string>>] => {
  const [value, setValue] = useState((propsValue && propsValue.isMark === true) ? 'block' : 'none');
  React.useEffect(() => {
    setValue((propsValue && propsValue.isMark === true) ? 'block' : 'none');
  }, [propsValue]);

  return [value, setValue];
};

interface IProps {
  note: INoteDTO | null | undefined;
  setNoteTab: (tab: string) => void;
  searchParams: URLSearchParams;
  setSearchParams: SetURLSearchParams;
  user: IUserDTO | null | undefined;
}

const WidgetMap: React.FunctionComponent<IProps> = (props) => {
  const {setNoteTab, note, searchParams, setSearchParams, user} = props;

  const [mapStateCache, setMapStateCache] = useLocalStorage<IMapState>(
    LocalStorageParams.MAPSTATE,
    { 
      zoom: DEFAULTMAPSTATE.zoom, 
      long: DEFAULTMAPSTATE.long,
      lat: DEFAULTMAPSTATE.lat
    });
  const [mapState, setMapState] = useState<IMapState>(mapStateCache);
  const markers = useFetch<Array<INoteDTO>>(getMarkersRequest(mapState, searchParams.get(URLParams.SEARCHQUERY), convertStringToInt(searchParams.get(URLParams.TAGSUM), 0), user), []);
  const [selectedNote, setSelectedNote] = useSelectedNote<INoteDTO | null | undefined>(note);
  const [popupDisplay, setPopupDisplay] = usePopupDisplay(note);
  const center = getCenter(note);
  const noteCode = searchParams.get(URLParams.NOTECODE);
  
  const handleMarkerClick = (marker: INoteDTO) => {
    setSelectedNote(marker);
    setPopupDisplay('block');
  };

  const handlePopupClick = (noteCode: string) => {
    setNoteTab(Tabs.NOTE);
    searchParams.set(URLParams.NOTECODE, noteCode); 
    setSearchParams(searchParams);
    setMapStateCache({ 
      zoom: mapState.zoom, 
      long: center[1], 
      lat: center[0]
    });
  };

  const handleBoundsChanged = (centerN: [number, number], zoomN: number) => {
    const deltaLat = 0.00009 * Math.pow(2, 19 - zoomN);
    const deltaLon = deltaLat * 1.5;
    if (zoomN !== mapState.zoom 
      || Math.abs(centerN[0] - mapState.lat) > deltaLat 
      || Math.abs(centerN[1] - mapState.long) > deltaLon)
    {
      setMapState(
        { 
          zoom: zoomN, 
          long: centerN[1], 
          lat: centerN[0]
        });
    }
  };

  return (
    <Map
      defaultCenter={[mapStateCache.lat, mapStateCache.long]} 
      defaultZoom={mapStateCache.zoom}
      center={center}
      onBoundsChanged={({ center, zoom }) => { 
        handleBoundsChanged(center, zoom)
      }}
    >
      <ZoomControl />
      {markers.map(marker => (
        <Marker
          key={marker.code}
          width={50}
          anchor={[marker.latitude, marker.longitude]} 
          color={noteCode === marker.code ? "#52ab98" : "#2b6777"}  
          onClick={() => handleMarkerClick(marker)}
        />
      ))}
      {
        selectedNote &&
          <Overlay 
            anchor={[selectedNote.latitude, selectedNote.longitude]} 
            offset={[86, -8]}
          >
            <div className="w4-marker-popup" style={{display: popupDisplay}}>
              <div className="w4-link"  onClick={() => handlePopupClick(selectedNote.code)}>
                {selectedNote.title}
              </div>
              <div>
                {selectedNote.description}
              </div>
              <div style={{display: 'flex', justifyContent: 'flex-end'}}>
                <div className="w4-button w4-button-marker" onClick={() => setPopupDisplay('none')}>
                  {CloseIcon}
                </div>
              </div>               
            </div>
          </Overlay>
      }       
    </Map>
  );
}

export default WidgetMap;