import React from 'react';
import { ITagDTO } from '../../types';


interface IProps {
  tagArray: Array<ITagDTO>;
  handleTagSelect: (tag: number) => void;
}

const TagListUnselected: React.FunctionComponent<IProps> = (props) => {
  return (
    <div className="w4-flex-wrap">
      {props.tagArray.map(tag => (
        <div key={tag.number} className="w4-button-primary w4-button-tag w4-tag w4-theme w4-flex" onClick={() => props.handleTagSelect(tag.number)}>
          <div className="w4-tag-text"> {tag.title} </div>
        </div>
      ))}
    </div>
  );
}

export default TagListUnselected;