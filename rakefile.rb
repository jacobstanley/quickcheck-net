# don't write the command to be executed by 'sh' to stdout
verbose(false)

MSBUILD    = 'c:/windows/microsoft.net/framework/v4.0.30319/msbuild.exe'
MONOLINKER = 'tools/monolinker/monolinker.exe'
ILREPACK   = 'tools/ilrepack/ilrepack.exe'

def tasks
  #task :default => [:copy_libs, :build, :link, :merge, :release]
  task :default => [:copy_libs, :build, :merge, :release]

  task :rebuild => [:clean, :default]

  task :clean do
    puts 'clean: cleaning up previous build output'
    stopwatch 'clean: clean up' do
      rm_rf FileList['bin/*']
      rm_rf FileList['src/**/bin']
      rm_rf FileList['src/**/obj']
    end
  end

  task :copy_libs do
    puts 'copy: lib -> bin/build'
    stopwatch 'copy: copying' do
      mkdir_p 'bin/build'
      cp_r FileList['lib/*'], 'bin/build'
    end
  end

  task :build do
    msbuild 'release', 'src/QuickCheck.sln'
  end

  #task :link do
  #  mkdir_p 'bin/link'

  #  link 'bin/build/QuickCheck', 'bin/link/QuickCheck'
  #end

  task :merge do
    mkdir_p 'bin/merge'

    merge 'bin/merge/QuickCheck.dll',
    #  FileList['bin/link/QuickCheck/*.dll'].
       ['bin/build/QuickCheck.dll',
        'bin/build/SimpleInjector.dll',
        'bin/build/SimpleInjector.Extensions.dll']
  end

  task :release do
    mkdir_p 'bin/release'

    files = FileList['bin/build/QuickCheck.*.dll'] +
            FileList['bin/merge/QuickCheck.dll']

    puts 'release: copying final assemblies to bin/release'
    stopwatch('release: copying') do
      puts files
      cp files, 'bin/release'
    end
  end

  def stopwatch(operation='operation')
    startTime = Time.now
    yield
    endTime = Time.now

    duration = endTime - startTime
    printf("#{operation} took %.1fs\n", duration)
  end

  def msbuild(config, project)
    cmd = "#{File.expand_path MSBUILD}"
    cmd << " #{project}"
    cmd << " /nologo"
    cmd << " /maxcpucount"
    cmd << " /verbosity:minimal"
    cmd << " /property:configuration=#{config}"
    cmd << " /target:build"

    puts "msbuild: building #{project} (#{config} mode)"
    stopwatch 'msbuild: building' do
      sh cmd
    end
  end

  def link(input, target)
    input_dir = File.dirname input
    input_asm = File.basename input

    cmd = "#{File.expand_path MONOLINKER}"
    cmd << " -a #{input_asm}"
    cmd << " -l none"
    cmd << " -o #{target}"
    cmd << " -d #{input_dir}"

    puts "monolinker: linking #{input}"
    stopwatch('monolinker: linking') do
      sh cmd
    end
  end

  def merge(target, inputs)
    cmd = "#{File.expand_path ILREPACK}"
    cmd << ' --internalize'
    cmd << ' --ndebug' # disable pdb files
    cmd << " --out:\"#{File.expand_path target}\""
    cmd << ' ' + inputs.map {|x| "\"#{x}\""}.join(' ')

    puts "ilrepack: #{target}"
    stopwatch('ilrepack: merge') do
      sh cmd
    end
  end
end

tasks
