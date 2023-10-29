import React from 'react';
import { useState } from 'react';
import { SetURLSearchParams } from "react-router-dom";
import { useFetch } from '../../customHooks';
import { Tabs, URLParams } from '../../constants';
import { ITagDTO, INoteDTO, IUserDTO } from '../../types';
import TagListSelected from '../lists/tag-selected-list';
import NoteList from '../lists/note-list';
import TagListUnselected from '../lists/tag-unselected-list';
import { FilterIcon } from '../icons/icons';
import { convertStringToInt } from '../../utils';


const getSearchRequest = (searchQuery: string | null, tagSum: number, user: IUserDTO | null | undefined): string | null => {
  if (user) {
    return `api/notes?user=${user.name}&query=${searchQuery || ''}&tags=${tagSum}`;
  }
  return null;
};

const getSelectedTags = (tagArray: Array<ITagDTO>, tagsNumber: number): Array<ITagDTO> => {
  let result: Array<ITagDTO> = [];
  tagArray.forEach(tag => {
    if ((tagsNumber & tag.number) === tag.number) {
      result.push({ number: tag.number, title: tag.title});
    }
  });
  return result;
};

const getUnselectedTags = (tagArray: Array<ITagDTO>, tagsNumber: number): Array<ITagDTO> => {
  let result: Array<ITagDTO> = tagArray.map(tag => tag);
  tagArray.forEach(tag => {
    if ((tagsNumber & tag.number) === tag.number) {
      result = result.filter(item => item.number !== tag.number);
    }
  });
  return result;
};

interface IProps {
  setNoteTab: (tab: string) => void;
  searchParams: URLSearchParams;
  setSearchParams: SetURLSearchParams;
  tagArray: Array<ITagDTO>;
  user: IUserDTO | null | undefined;
}

const WidgetSearch: React.FunctionComponent<IProps> = (props) => {
  const { setNoteTab, searchParams, setSearchParams, tagArray, user } = props;

  const isIndex = window.location.pathname === "/";
  const [searchQuery, setSearchQuery] = useState(searchParams.get(URLParams.SEARCHQUERY) || '');
  const [tagSum, setTagSum] = useState(convertStringToInt(searchParams.get(URLParams.TAGSUM), 0));
  const searchResult = useFetch<Array<INoteDTO>>(getSearchRequest(searchQuery, tagSum, user), []);
  
  const tagArraySelected = getSelectedTags(tagArray, tagSum);
  const tagArrayUnselected = getUnselectedTags(tagArray, tagSum);

  const [showFilterModal, setShowFilterModal] = useState('none');

  const handleInputKeyDown = (event: React.KeyboardEvent<HTMLInputElement>): void => {
    if (event.key === 'Enter') {
      setSearchQuery(event.currentTarget.value);
      searchParams.set(URLParams.SEARCHQUERY, event.currentTarget.value);
      setSearchParams(searchParams);
    }
  };

  const changeTagSum = (newTagSum: number): void => {
    searchParams.set(URLParams.TAGSUM, newTagSum.toString()); 
    setSearchParams(searchParams);
    setTagSum(newTagSum);
  }

  const handleNoteClick = (noteCode: string): void => {
    setNoteTab(Tabs.NOTE);
    if (isIndex){
      window.location.href = window.location.href + noteCode;
    } else {
      searchParams.set(URLParams.NOTECODE, noteCode);
      setSearchParams(searchParams);
    }
  };

  return (
    <div className="w4-widget w4-flex-column">
      <div className="w4-widget__head w4-container w4-theme-cyan">
        <div className="w4-flex w4-margin-top w4-margin-bottom" style={{position: "relative"}}>
          <div className="w4-widget__head__input">
            <input 
              className="w4-input w4-input--search" 
              type="text"
              placeholder={isIndex ? "search users" : "search notes"}
              onKeyDown={handleInputKeyDown}
            />
          </div>
          <div className="w4-button w4-button-primary w4-widget__head__button w4-theme" onClick={() => setShowFilterModal('block')}>
            {FilterIcon}              
          </div>
          <div className="w4-widget__modal w4-widget__modal--filter w4-theme-text w4-container" style={{display: showFilterModal, zIndex:'10'}}>
            <h2>Tags:</h2>
            <TagListUnselected 
              tagArray={tagArrayUnselected} 
              handleTagSelect={(tag) => changeTagSum(tagSum + tag)} 
            />
          </div>
          <div className="w4-modal-background" 
            onClick={() => setShowFilterModal('none')} 
            style={{display: showFilterModal}} 
          />
        </div>
        <TagListSelected 
          tagArray={tagArraySelected} 
          handleTagUnselect={(tag) => changeTagSum(tagSum - tag)}
        />
      </div>
      <div className="w4-widget__body w4-container w4-theme-text">
        <NoteList 
          searchResult={searchResult}
          handleNoteClick={handleNoteClick}
        />
      </div>
    </div>
  );
}

export default WidgetSearch;