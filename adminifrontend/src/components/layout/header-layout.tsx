import AdminiLogo from "../../assets/files/logo.svg";

interface IProps {
  header: string;
  children?: JSX.Element;
}

export const HeaderLayout: React.FunctionComponent<IProps> = ({ header, children }) => {
  return (
    <header className="w4-header w4-theme w4-flex w4-flex-column">
      <div className="w4-navbar w4-flex">
        <div className="w4-flex">
          <a href="/" className="w4-button w4-logo">
            <img src={AdminiLogo} alt="Admini logo" width="26" />
          </a>
          {
            header &&
            <a href={`/${header}`} className="w4-button w4-logo" style={{paddingLeft:"0px"}}>
              {header} 
            </a>
          }  
        </div>     
        {children}
      </div>
    </header>
  );
};