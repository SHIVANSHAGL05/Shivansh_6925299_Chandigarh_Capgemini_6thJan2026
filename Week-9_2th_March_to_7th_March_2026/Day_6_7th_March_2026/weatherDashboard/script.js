document.getElementById("checkBtn").onclick=function(){

var city=document.getElementById("city").value

var weather="Sunny"
var temp="22°C"

document.getElementById("result").innerHTML=
"City: "+city+"<br>Temp: "+temp+"<br>Weather: "+weather

if(weather==="Sunny"){
document.body.style.background="yellow"
}
else if(weather==="Rain"){
document.body.style.background="lightblue"
}
else{
document.body.style.background="gray"
}

}