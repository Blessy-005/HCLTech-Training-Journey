import { Component } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-movie-details',
  imports: [RouterLink],
  templateUrl: './movie-details.html',
  styleUrl: './movie-details.css'
})
export class MovieDetails {
  movieId = '';

  movie = {
    title: 'Interstellar',
    genre: 'Sci-Fi / Adventure',
    language: 'English',
    duration: '2h 49m',
    releaseDate: '2014',
    rating: '8.7 / 10',
    description: 'A team of explorers travels beyond this galaxy to discover whether mankind has a future among the stars.',
    poster: 'https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg'
  };

  constructor(private route: ActivatedRoute) {
    this.movieId = this.route.snapshot.paramMap.get('id') ?? '';
  }

  shows = [
    { theatre: 'PVR Cinemas', time: '10:30 AM', price: 180 },
    { theatre: 'INOX', time: '1:45 PM', price: 200 },
    { theatre: 'AGS Cinemas', time: '6:30 PM', price: 160 },
    { theatre: 'PVR Cinemas', time: '9:45 PM', price: 220 }
  ];
}

