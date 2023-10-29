const tagsElement = document.getElementById("tags");

const setSelectedTags = (number) => {
  const tagElements = document.getElementsByClassName("w3-check");
  Array.from(tagElements, (element) => {
    if ((number & element.value) === parseInt(element.value)) {
      element.checked = true;
    }
  })
};

const updateTagsSum = (element) => {
  if (element.checked) {
    tagsElement.value = parseInt(tagsElement.value) + parseInt(element.value);
    return;
  }
  tagsElement.value = parseInt(tagsElement.value) - parseInt(element.value);
};

const clearTags = () => {
  tagsElement.value = 0;
  const tagElements = document.getElementsByClassName("w3-check");
  Array.from(tagElements, (element) => {
    element.checked = false;
  })
};

setSelectedTags(tagsElement.value);