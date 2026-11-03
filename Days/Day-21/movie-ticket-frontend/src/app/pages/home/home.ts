import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

  movies = [
    {
      title: 'Leo',
      genre: 'Action',
      language: 'Tamil',
      duration: '2h 44m',
      poster: 'https://upload.wikimedia.org/wikipedia/commons/thumb/8/83/Leo-japanese-posters-v0-tjnsem1799wb1.png/500px-Leo-japanese-posters-v0-tjnsem1799wb1.png'
    },
    {
      title: 'Jailer',
      genre: 'Action',
      language: 'Tamil',
      duration: '2h 48m',
      poster: 'https://m.media-amazon.com/images/M/MV5BZTM3NDEwYWYtZjljZC00Y2FkLTgzZDEtNjhhMjQwN2Q3MTMwXkEyXkFqcGc@._V1_.jpg'
    },

    {
      title: 'Interstellar',
      genre: 'Sci-Fi',
      language: 'English',
      duration: '2h 49m',
      poster: 'https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg'
    },
    {
      title: 'Inception',
      genre: 'Thriller',
      language: 'English',
      duration: '2h 28m',
      poster: 'https://image.tmdb.org/t/p/w500/oYuLEt3zVCKq57qu2F8dT7NIa6f.jpg'
    }
  ];

  exploreMovies() {
    document.getElementById('movies')?.scrollIntoView({
      behavior: 'smooth'
    });
  }
}



