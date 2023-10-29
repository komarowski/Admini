import React from 'react';
import { INoteDTO } from '../../types';
import { getFormatDateString } from '../../utils';


interface IProps {
  searchResult: Array<INoteDTO>;
  handleNoteClick: (code: string) => void;
}

const NoteList: React.FunctionComponent<IProps> = (props) => {
  return (
    <>
      {props.searchResult.map(note => (
        <div key={note.code} className="w4-search-result" onClick={() => props.handleNoteClick(note.code)}>
          <div className="w4-flex">
            <span className="w4-dot" />
            <h2>{note.title}</h2>
          </div>
          <div className="w4-search-result-container">
            <div className="w4-search-result__date"> {getFormatDateString(note.lastUpdate)} </div>
            <div> {note.description} </div>
          </div>
        </div>
      ))}
    </>
  );
}

export default NoteList;