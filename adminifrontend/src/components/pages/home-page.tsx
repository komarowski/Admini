import React from 'react';
import { useSearchParams } from "react-router-dom";
import WidgetSearch from "../widgets/widget-search";
import WidgetNote from "../widgets/widget-note";
import WidgetMap from "../widgets/widget-map";
import { Tabs, URLParams } from '../../constants';
import { SearchIcon, NoteIcon, MapIcon } from '../icons/icons';
import { INoteDTO, ITagDTO, IUserDTO } from '../../types';
import { useFetch } from '../../customHooks';
import { HeaderLayout } from '../layout/header-layout';


const HomePage: React.FunctionComponent = () => {
  const WidgetSearchRef = React.useRef<HTMLDivElement>(null);
  const WidgetNoteRef = React.useRef<HTMLDivElement>(null);
  const WidgetMapRef = React.useRef<HTMLDivElement>(null);
  const TabSearchRef = React.useRef<HTMLDivElement>(null);
  const TabNoteRef = React.useRef<HTMLDivElement>(null);
  const TabMapRef = React.useRef<HTMLDivElement>(null);

  const [searchParams, setSearchParams] = useSearchParams();
  const noteCode = searchParams.get(URLParams.NOTECODE) || 'index';
  const pathArray = window.location.pathname.split("/");
  const userName = pathArray[1].toLowerCase();

  const user = useFetch<IUserDTO | null | undefined>(`api/users?user=${userName}`, undefined);
  const note = useFetch<INoteDTO | null | undefined>(user ? `api/note?user=${userName}&code=${noteCode}` : null, undefined);
  const tagArray = useFetch<Array<ITagDTO>>(user ? `api/tags?user=${userName}` : null, []);

  if (pathArray.length >= 3) {
    window.history.replaceState(null, "Index", "/" + userName);
  }

  const setTab = (tab: string): void => {
    changeZIndex(WidgetSearchRef.current!, tab, Tabs.SEARCH);
    changeZIndex(WidgetNoteRef.current!, tab, Tabs.NOTE)
    changeZIndex(WidgetMapRef.current!, tab, Tabs.MAP)
    changeActiveClass(TabSearchRef.current!, tab, Tabs.SEARCH);
    changeActiveClass(TabNoteRef.current!, tab, Tabs.NOTE);
    changeActiveClass(TabMapRef.current!, tab, Tabs.MAP);
  }

  const changeActiveClass = (element: HTMLDivElement, currentTab: string, targetTab: string) => {
    currentTab === targetTab 
    ? element.classList.replace("w4-button-tab--inactive", "w4-button-tab--active") 
    : element.classList.replace("w4-button-tab--active", "w4-button-tab--inactive");
  }

  const changeZIndex = (element: HTMLDivElement, currentTab: string, targetTab: string) => {
    element.style.zIndex = targetTab === currentTab ? "2" : "1";
  }
 
  return (
    <>
      <HeaderLayout 
        header={userName} 
        children={
          <div className="w4-tab-container">
            <div ref={TabSearchRef} className='w4-button w4-button-tab w4-button-tab--inactive w4-button-tab--search' onClick={() => setTab(Tabs.SEARCH)}>
              {SearchIcon} &nbsp; Search
            </div>
            <div ref={TabNoteRef} className='w4-button w4-button-tab w4-button-tab--active' onClick={() => setTab(Tabs.NOTE)}>
              {NoteIcon} &nbsp; Note
            </div>   
            <div ref={TabMapRef} className='w4-button w4-button-tab w4-button-tab--inactive' onClick={() => setTab(Tabs.MAP)}>
              {MapIcon} &nbsp; Map
            </div>
          </div>
        }
      />
      <main className="w4-main">
        <div ref={WidgetSearchRef} className="w4-section w4-section--search">
          <WidgetSearch 
            setNoteTab={setTab} 
            searchParams={searchParams}
            setSearchParams={setSearchParams}
            tagArray={tagArray}
            user={user}
          />
        </div>
        <div ref={WidgetNoteRef} className="w4-section w4-section--note">
          <WidgetNote 
            user={user}
            note={note} 
          />
        </div>
        <div ref={WidgetMapRef} className="w4-section w4-section--map">
          <WidgetMap 
            note={note} 
            setNoteTab={setTab} 
            searchParams={searchParams}
            setSearchParams={setSearchParams}
            user={user}
          />
        </div>
      </main>
    </>
  )
}

export default HomePage;
