async function func(){
    let response=await fetch('./Info.txt')
    let text=await response.text()
    document.getElementById("text").innerHTML=text
}
func()