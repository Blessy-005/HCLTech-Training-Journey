import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Movie {
  id: number;
  title: string;
  description: string;
  language: string;
  genre: string;
  durationMinutes: number;
  posterUrl: string;
  releaseDate: string;
  isActive: boolean;
}

export interface MovieResponse {
  items: Movie[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

@Injectable({ providedIn: 'root' })
export class MovieService {
  private readonly apiUrl = 'http://localhost:5162/api/Movies';

  constructor(private http: HttpClient) {}

  getMovies(search = '', genre = ''): Observable<MovieResponse> {
    let params = new HttpParams()
      .set('pageNumber', '1')
      .set('pageSize', '100');

    if (search) params = params.set('search', search);
    if (genre && genre !== 'All') params = params.set('genre', genre);

    return this.http.get<MovieResponse>(this.apiUrl, { params });
  }

  getMovieById(id: number): Observable<Movie> {
    return this.http.get<Movie>(`${this.apiUrl}/${id}`);
  }
}
