export interface ITagDTO {
  number: number;
  title: string;
}

export interface IUserDTO {
  name: string;
  id: number;
}

export interface INoteDTO {
  code: string;
  title: string;
  description: string;
  tagsString: string;
  isMark: boolean;
  lastUpdate: string;
  latitude: number;
  longitude: number;
} 

export interface IMapState {
  zoom: number;
  long: number;
  lat: number;
}
