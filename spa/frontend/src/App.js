import logo from './logo.svg';
import './App.css';
import React, { useState } from 'react';
import ShowJson from './components/showJson';


function App() {
  const [result, setResult] = useState("");
  const [isError, setIsError] = useState(true);

  const invokeAPI = async () => {
    try {
      const response = await fetch("https://localhost:7242/WeatherForecast", {
        headers: {
        },
      });

      if (!response.ok) {
        let message = `Fetch failed with HTTP status ${response.status} ${response.statusText}`;
        setResult(message);
        setIsError(true);
        return;
      }

      setResult(await response.json());
      setIsError(false);
    }
    catch (e) {
      console.log(e);
      setResult(e.message);
      setIsError(true);
    }
  }



  return (
    <div >
      <header className="header">
        <div className="one">
          <a href="#" onClick={invokeAPI}>Invoke API</a>
        </div>
        <div className="two">

        </div>
        <div className="three">
          {/* this will only work when the react app is hosted by ASP.NET */}
          <a href="/swagger" >Swagger</a>
        </div>
      </header>

      <div className="content">
        <img src={logo} className="App-logo" alt="logo" />

      </div>

      <div className="apiResult">
        {isError ? result : (<ShowJson label="API result" data={result} />)}
      </div>

    </div>
  );
}

export default App;
