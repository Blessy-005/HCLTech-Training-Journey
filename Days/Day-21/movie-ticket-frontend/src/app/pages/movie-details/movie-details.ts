import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Movie, MovieService } from '../../services/movie';

@Component({
  selector: 'app-movie-details',
  imports: [RouterLink],
  templateUrl: './movie-details.html',
  styleUrl: './movie-details.css'
})
export class MovieDetails implements OnInit {
  movie: Movie | null = null;
  isLoading = false;
  errorMessage = '';

  shows = [
    { theatre: 'PVR Cinemas', time: '10:30 AM', price: 180 },
    { theatre: 'INOX', time: '1:45 PM', price: 200 },
    { theatre: 'AGS Cinemas', time: '6:30 PM', price: 160 },
    { theatre: 'PVR Cinemas', time: '9:45 PM', price: 220 }
  ];

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id || id <= 0) {
      this.errorMessage = 'Invalid movie ID.';
      return;
    }

    this.loadMovie(id);
  }

  loadMovie(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.movieService.getMovieById(id).subscribe({
      next: movie => {
        this.movie = movie;
        this.isLoading = false;
      },
      error: error => {
        console.error('Movie details API error:', error);
        this.errorMessage = 'Unable to load movie details.';
        this.isLoading = false;
      }
    });
  }

  getDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return `${hours}h ${remainingMinutes}m`;
  }

  getReleaseYear(date: string): string {
    return new Date(date).getFullYear().toString();
  }
}
