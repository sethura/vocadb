﻿
$(document).ready(function () {

	$("#globalSearchTerm").autocomplete({
		source: function (request, response) {

			var term = request.term;
			var entryType = $("input[@name = 'globalSearchEntryType']:checked").val(); // $("#globalSearchEntryType").val();

			if (entryType == "Album") {
				$.post("../../Album/FindNames", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			} else if (entryType == "Artist") {
				$.post("../../Artist/FindNames", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			} else if (entryType == "Song") {
				$.post("../../Song/FindNames", { term: term }, function (results) {
					entryFindCallback(response, results);
				});
			}

		}
	});


});

function entryFindCallback(response, results) {

	response($.map(results, function( item ) {
		return item; 
	}));

}