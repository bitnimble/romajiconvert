let library = require("./output.json");

function search(input) {
	//Preprocess the search input to be only lowercase letters and numbers
	let newInput = [];
	for (let i = 0; i < input.length; i++) {
		let charVal = input[i].charCodeAt(0);
		if ((charVal >= 48 && charVal <= 57) || (charVal >= 97 && charVal <= 122)) {
			newInput.push(input[i]);
		} else if (charVal >= 65 && charVal <= 90) {
			newInput.push(input[i].toLowerCase());
		}
	}
	newInput = newInput.join("");
	
	let results = [];
	for (let song of library.songs) {
		if (song.tag.includes(newInput)) {
			results.push(song);
		}
	}
	
	return results;
}





//Example usage
let searchResults = search("KIMI		 NO     SHiRANaI");
for (let song of searchResults) {
	let animeTitle = song.anime.trim() == "" ? "" : " (" + song.anime + ")";
	console.log(song.id + " - " + song.title + " - " + song.artist + animeTitle);
}