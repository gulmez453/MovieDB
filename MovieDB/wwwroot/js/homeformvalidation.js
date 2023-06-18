function validateInput(inputId) {
    const textInput = document.getElementById(inputId);
    const inputValue = textInput.value.trim();

    // Remove any non-digit characters from the input
    const numericInput = inputValue.replace(/\D/g, '');

    // Update the input field with the cleaned numeric value
    textInput.value = numericInput;
}