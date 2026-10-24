function func() {
    let height = Number(document.getElementById("height").value);
    let weight = Number(document.getElementById("weight").value);

    if (height <= 0 || weight <= 0) {
        document.getElementById("res").innerHTML = "";
        return;
    }

    let bmi = (weight / height / height) * 10000;
    let condition = "";

    if (bmi < 18.5) {
        condition = "Underweight";
    } else if (bmi < 25) {
        condition = "Normal weight";
    } else if (bmi < 30) {
        condition = "Overweight";
    } else {
        condition = "Obesity";
    }

    document.getElementById("res").innerHTML =
        "Your BMI is " + bmi.toFixed(2) + " - " + condition;
}