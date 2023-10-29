import React from 'react';
import { ITagDTO } from '../../types';
import { CloseIcon } from '../icons/icons';


interface IProps {
  tagArray: Array<ITagDTO>;
  handleTagUnselect: (tag: number) => void;
}

const TagListSelected: React.FunctionComponent<IProps> = (props) => {
  return (
    <div className="w4-flex-wrap">
      {props.tagArray.map(tag => (
        <div key={tag.number} className="w4-tag w4-theme w4-flex">
          <div className="w4-tag-text"> {tag.title} </div>
          <div className="w4-button w4-button-tag-close" onClick={() => props.handleTagUnselect(tag.number)}>
            {CloseIcon}
          </div>
        </div>
      ))}
    </div>
  );
}

export default TagListSelected;