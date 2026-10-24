function map() {
    let name = document.getElementById("name").value;
    let age = Number(document.getElementById("age").value);
    let data1=new Map()
    data1.set("Name",name)
    data1.set("Age",age)
    let data = {
        "Name": name,
        "Age": age
    };

    document.getElementById("res").innerHTML =
        "The entered data are: " + JSON.stringify(data);
    document.getElementById("res1").innerHTML="The entered data are: " +JSON.stringify(Object.fromEntries(data1))
    console.log(data);
    console.log(data1)
}