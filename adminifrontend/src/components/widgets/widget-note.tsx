import React, { useEffect } from 'react';
import { useFetch } from '../../customHooks';
import { INoteDTO, IUserDTO } from '../../types';
import { getFormatDateString } from '../../utils';


const WidgetNoteState = {
  UNDEFINED: 0,
  USERNOTFOUND: 1,
  NOTENOTFOUND: 2,
  FOUND: 3,
};

const getWidgetNoteState = (note: INoteDTO | null | undefined, user: IUserDTO | null | undefined): number => {
  if (user === undefined){
    return WidgetNoteState.UNDEFINED;
  }
  if (user === null){
    return WidgetNoteState.USERNOTFOUND;
  }
  if (note === undefined){
    return WidgetNoteState.UNDEFINED;
  }
  if (note === null){
    return WidgetNoteState.NOTENOTFOUND;
  }
  return WidgetNoteState.FOUND;
}

interface IProps {
  note: INoteDTO | null | undefined;
  user: IUserDTO | null | undefined;
}

const WidgetNote: React.FunctionComponent<IProps> = (props) => {
  const {user, note} = props;
  const noteHtml = useFetch<string>((note && user) ? `notes/${user.name}/${note.code}/index.txt` : null, '', true);
  const state = getWidgetNoteState(note, user);

  const setSlides = (slides: NodeListOf<HTMLElement>, curSlide: number): void => {
    slides.forEach((slide, indx) => {
      slide.style.transform = `translateX(${(indx - curSlide) * 100}%)`;
    });
  }

  useEffect(() => {
    document.querySelectorAll(".w4-slider").forEach((slider) => {
      const slides = slider.querySelectorAll<HTMLElement>(".w4-slide");
      const nextSlide = slider.querySelector(".w4-button-slider--next") as HTMLElement;
      const prevSlide = slider.querySelector(".w4-button-slider--prev") as HTMLElement;
      const maxSlideIndex = slides.length - 1;
      let curSlideIndex = 0;
      if (nextSlide) {
        nextSlide.onclick = () => {
          curSlideIndex = (curSlideIndex === maxSlideIndex) ? 0 : curSlideIndex + 1;
          setSlides(slides, curSlideIndex);
        };
      }
      if (prevSlide) {
        prevSlide.onclick = () => {
          curSlideIndex = (curSlideIndex === 0) ? maxSlideIndex : curSlideIndex - 1;
          setSlides(slides, curSlideIndex);
        };
      }
      setSlides(slides, 0);
    });
  }, [noteHtml]);

  return (
    <div className="w4-widget w4-flex-column">
      <div className="w4-widget__body w4-theme-text">
        <div className="w4-container-note">
          {
            (() => {
              switch(state) {
                case WidgetNoteState.FOUND:
                  return (
                    <>
                      <div className="w4-flex-beetwen" style={{alignItems: "flex-start"}}>
                        <h1>{note!.title}</h1>
                        <div style={{minWidth: '100px', margin: "5px 0"}}> {getFormatDateString(note!.lastUpdate)}</div>
                      </div>
                      {
                        note!.tagsString &&
                        <div className="w4-flex">
                          <div className="w4-tag w4-tag--string w4-theme w4-flex" style={{fontSize: "14px"}}>
                            {note!.tagsString}
                          </div>
                        </div>  
                      }
                      <div className="w4-blog" dangerouslySetInnerHTML={{__html: noteHtml}} />
                    </>
                  )
                case WidgetNoteState.USERNOTFOUND:
                  return (
                    <h1>{`User "${window.location.pathname.split("/")[1].toLowerCase()}" not found!`}</h1>
                  )
                case WidgetNoteState.NOTENOTFOUND:
                  return (
                    <h1>{`Note "${`notes/${user?.name}/${note?.code}`}" not found!`}</h1>
                  )
                default:
                  return <></>
              }
            })()
          }
        </div>
      </div>
    </div>
  );
}

export default WidgetNote;