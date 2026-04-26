document.getElementById("addBtn").onclick=function(){

var task=document.getElementById("taskInput").value

if(task==="") return

var li=document.createElement("li")

li.innerText=task

li.onclick=function(){
this.classList.toggle("done")
}

document.getElementById("taskList").appendChild(li)

document.getElementById("taskInput").value=""

}

document.getElementById("resetBtn").onclick=function(){

document.getElementById("taskList").innerHTML=""

}